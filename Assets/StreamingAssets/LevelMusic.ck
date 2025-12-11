global string level;
global Event LevelStarted;
global float freqs[16];
16 => int FFT_SIZE;

SndBuf buffer => FFT fft => blackhole; // FFT analysis chain
buffer => dac; // Audio output chain

FFT_SIZE => fft.size;
Windowing.hann(FFT_SIZE) => fft.window;

while(true) {
    LevelStarted => now;
    
    "/Resources/" + level + "/song.mp3" => buffer.read;
    <<< "/Resources/" + level + "/song.mp3" >>>;
    0 => buffer.pos;
    
    // Advance time to allow FFT to analyze
    FFT_SIZE::samp => now;
    
    // Get FFT data
    fft.upchuck();
    
    // Extract magnitudes into array
    for(0 => int i; i < FFT_SIZE/2; i++) {
        fft.cval(i) $ polar @=> polar p;
        p.mag => freqs[i];
    }
    
    // Continue playing the rest of the file
    buffer.length() - (FFT_SIZE::samp) => now;
}