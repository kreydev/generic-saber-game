global string level;
global float freqs[512];
512 => int FFT_SIZE;

global Event LevelDone;

SndBuf buffer => FFT fft => blackhole;
buffer => dac;

FFT_SIZE => fft.size;
Windowing.hann(FFT_SIZE) => fft.window;

level + ".wav" => buffer.read;
0 => buffer.pos;

// Analyze continuously while playing
while(buffer.pos() < buffer.samples()) {
    // Advance time by hop size
    (FFT_SIZE/2)::samp => now;
    
    // Get FFT data
    fft.upchuck();
    
    // Extract magnitudes into array
    for(0 => int i; i < FFT_SIZE/2; i++) {
        fft.cval(i) $ polar @=> polar p;
        p.mag => freqs[i];
    }
}

LevelDone.broadcast();