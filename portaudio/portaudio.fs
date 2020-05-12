c-library portaudio
s" portaudio" add-lib
\c #include <portaudio.h>
c-function pa-get-version Pa_GetVersion -- n
c-function pa-sleep Pa_Sleep n -- void
c-function pa-initialize Pa_Initialize -- n
c-function pa-terminate Pa_Terminate -- n
\ PaError Pa_OpenDefaultStream ( PaStream **  stream, int  numInputChannels, int  numOutputChannels, PaSampleFormat  sampleFormat, double  sampleRate, unsigned long  framesPerBuffer, PaStreamCallback *  streamCallback, void *  userData )
c-function pa-open-default-stream Pa_OpenDefaultStream a n n n r n func a -- n
c-function pa-start-stream Pa_StartStream a -- n
c-function pa-stop-stream Pa_StopStream a -- n
c-function pa-close-stream Pa_CloseStream a -- n
\ typedef int PaStreamCallback(const void *input, void *output, unsigned long frameCount, const PaStreamCallbackTimeInfo *timeInfo, PaStreamCallbackFlags statusFlags, void *userData)
c-callback pa-stream-callback: a a n a n a -- n
end-c-library
