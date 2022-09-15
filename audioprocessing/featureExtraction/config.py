# not up to date

sr = 44100
frameSize = 2048
hopSize = 1024

music_path = '/data/Music'
json_path = '/data/JsonFiles'
csv_path = '/data'

tempoCNN = '/data/Models/bpm.pb'
genreCNN = '/data/Models/genre.pb'

otherCNNs = ['/data/Models/voice.pb',
             '/data/Models/female.pb',
             '/data/Models/danceable.pb',
             '/data/Models/tonal.pb']

# put everything here that needs to be extracted by Essentias MusicExtractor
featuresMusicExtractor = [

    'lowlevel.spectral_centroid.max', 'lowlevel.spectral_centroid.min', 'lowlevel.spectral_centroid.median', 'lowlevel.spectral_centroid.mean', 'lowlevel.spectral_centroid.stdev',

    'lowlevel.spectral_rolloff.max', 'lowlevel.spectral_rolloff.min', 'lowlevel.spectral_rolloff.median', 'lowlevel.spectral_rolloff.mean', 'lowlevel.spectral_rolloff.stdev',

    'lowlevel.spectral_flux.max', 'lowlevel.spectral_flux.min', 'lowlevel.spectral_flux.median', 'lowlevel.spectral_flux.mean', 'lowlevel.spectral_flux.stdev',

    'lowlevel.melbands_crest.max', 'lowlevel.melbands_crest.min', 'lowlevel.melbands_crest.median', 'lowlevel.melbands_crest.mean', 'lowlevel.melbands_crest.stdev',

    'lowlevel.mfcc.mean'
]

# additional features, that are not in Essentias MusicExtractor (13 mfccs are in MusicExtractor, but are getting individual names here)
featuresAdditional = ['mfcc_1', 'mfcc_2', 'mfcc_3', 'mfcc_4', 'mfcc_5', 'mfcc_6', 'mfcc_7', 'mfcc_8',
                      'mfcc_9', 'mfcc_10', 'mfcc_11', 'mfcc_12', 'mfcc_13',
                      'rmsWaveForm', 'bpm', 'danceable', 'voice', 'tonal', 'female']


merge = [*featuresMusicExtractor, *featuresAdditional]
allFeatures = merge.remove('lowlevel.mfcc.mean')


aditional = [
    'lowlevel.spectral_complexity.max', 'lowlevel.spectral_complexity.median', 'lowlevel.spectral_complexity.mean', 'lowlevel.spectral_complexity.stdev',

    'lowlevel.dynamic_complexity',

    'lowlevel.pitch_salience.max', 'lowlevel.pitch_salience.min', 'lowlevel.pitch_salience.median', 'lowlevel.pitch_salience.mean', 'lowlevel.pitch_salience.stdev',

    'lowlevel.melbands_flatness_db.max', 'lowlevel.melbands_flatness_db.min', 'lowlevel.melbands_flatness_db.median', 'lowlevel.melbands_flatness_db.mean', 'lowlevel.melbands_flatness_db.stdev',

    'lowlevel.zerocrossingrate.max', 'lowlevel.zerocrossingrate.min', 'lowlevel.zerocrossingrate.median', 'lowlevel.zerocrossingrate.mean', 'lowlevel.zerocrossingrate.stdev',

    'rhythm.onset_rate', 'rhythm.beats_count', 'tonal.chords_changes_rate', 'tonal.chords_number_rate',

    'tonal.chords_strength.max', 'tonal.chords_strength.min', 'tonal.chords_strength.median', 'tonal.chords_strength.mean', 'tonal.chords_strength.stdev',

    'rhythm.danceability',]
