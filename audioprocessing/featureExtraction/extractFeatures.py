# not up to date

from essentia import *
from essentia.standard import *
import config as config
import pandas as pd
import numpy as np
import heapq
import tensorflow as tf
import scipy.stats as stats
import os

# Instantiate Essentias RMS Algorithm
pool = Pool()
dur = Duration()
rms = RMS()

def storeToCSV(data):
    # storing to csv
    data.to_csv(dataset_path + '/' + 'extraction.csv', index=False, mode='a', header=(not os.path.exists(dataset_path + '/' + 'extraction.csv')))
    extracted_Data.clear()

def extractor(file, filename):

    # run ML-Models
    features, features_frames, emotionML = deepLClassifier(file)

    # extract LowLevel-Features
    duration, extractedData = essentiaExtractor(file)

    return duration, features, features_frames


def essentiaExtractor(file):

    extracted_Data = []

    features, features_frames = MusicExtractor(lowlevelStats=['mean', 'stdev', 'min', 'max', 'median'],
                                               rhythmStats=['mean', 'stdev', 'min', 'max', 'median'],
                                               tonalStats=['mean', 'stdev', 'min', 'max', 'median'],
                                               analysisSampleRate=config.sr, lowlevelHopSize=config.hopSize,
                                               lowlevelFrameSize=config.frameSize, tonalHopSize=config.hopSize,
                                               tonalFrameSize=config.frameSize)(file)

    # add MusicExtractor features
    for i in config.featuresMusicExtractor:
        if i == 'lowlevel.mfcc.mean':
            for m in range(0, 13):
                mfcc = features['lowlevel.mfcc.mean'][m]
                extracted_Data.append(mfcc)
        else:
            extracted_Data.append(features[i])

    audioFile = MonoLoader(filename=file, sampleRate=config.sr)()
    rmsWaveform = rms(audioFile)
    extracted_Data.append(rmsWaveform)

    duration = dur(audioFile)

    return duration, extracted_Data

def deepLClassifier(file):
    features = {}
    features_frames = {}

    # for emotion models
    emotionData = []

    # bpm cnn works with 11 khz? Others with 16 khz
    audio_11khz = MonoLoader(filename=file, sampleRate=11025)()
    audio = MonoLoader(filename=file, sampleRate=16000)()

    # get BPM
    global_bpm, local_bpm, local_probs = TempoCNN(graphFilename=config.tempoCNN)(audio_11khz)
    features['bpm'] = round(float(global_bpm), 8)

    emotionData.append([global_bpm])

    # extract danceable, voice, tonal, female , bpm
    highLevel = {}
    for i, name in enumerate(config.otherCNNs):

        dict_name = name.split('/')[-1].split('.')[0]
        activations = TensorflowPredictMusiCNN(graphFilename=name)(audio)
        predictions = list(np.mean(activations, axis=0))

        if(dict_name != 'tonal'):
            emotionData.append(predictions)

        if(dict_name == 'voice'):
            features[dict_name] = round(float(predictions[1]), 8)
            highLevel[dict_name] = list(np.around(activations[:, 1].astype(float), decimals=8))
        else:
            features[dict_name] = round(float(predictions[0]), 8)
            highLevel[dict_name] = list(np.around(activations[:, 0].astype(float), decimals=8))

        features_frames['highLevel_graphs'] = highLevel

    #extract genre
    activations = TensorflowPredictMusiCNN(graphFilename=config.genreCNN)(audio)
    predictions = list(np.around(np.mean(activations, axis=0).astype(float), decimals=8))

    # delete unecessary genres
    indexes = [5, 7, 10, 13, 17, 19, 21, 25, 31, 32, 34, 35, 38, 39, 44, 47, 49]
    for index in sorted(indexes, reverse=True):
        del predictions[index]
    features['genres'] = predictions

    # delete unecessary genres from frames
    genre_frames = []
    for i, frame in enumerate(activations):
        for index in sorted(indexes, reverse=True):
            frame = list(np.around(np.delete(frame, index).astype(float), decimals=8))
        genre_frames.append(frame)

    # extract top 3 genres and the genre graphs
    topGenre = heapq.nlargest(3, ((x, i) for i, x in enumerate(predictions)))
    genre_graphs = {}
    for genre in topGenre:
        frames = []
        for i, frame in enumerate(genre_frames):
            frames.append(genre_frames[i][genre[1]])
        genre_graphs[str(genre[1])] = frames
    features_frames['genre_graphs'] = genre_graphs

    return features, features_frames, emotionData


