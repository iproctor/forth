require orchestrator.fs
require single-cycle.fs
require envelope.fs
require sample_instrument.fs
require delay.fs

120 to bpm

0  s" samples/KickDrum/" load-samples drop 0.3e new-samples-instrument constant kickdrum

0.5e sin-sample \sin-sample new-single-cycle-instrument constant sin-inst
0.5e square-sample \square-sample new-single-cycle-instrument constant square-inst
0.5e saw-sample \square-sample new-single-cycle-instrument constant saw-inst
0.5e  s" waveforms/AKWF/AKWF_cello/AKWF_cello_0002.wav" load-single-cycle  new-single-cycle-instrument constant cello-inst
0.5e  s" waveforms/AKWF/AKWF_fmsynth/AKWF_fmsynth_0013.wav" load-single-cycle  new-single-cycle-instrument constant fmsynth-inst
0.5e  s" waveforms/AKWF/AKWF_hvoice/AKWF_hvoice_0007.wav" load-single-cycle  new-single-cycle-instrument constant hvoice-inst
0.5e  s" waveforms/AKWF/AKWF_piano/AKWF_piano_0001.wav" load-single-cycle  new-single-cycle-instrument constant piano-inst
0.2e  s" waveforms/AKWF/AKWF_granular/AKWF_granular_0006.wav" load-single-cycle  new-single-cycle-instrument constant granular-inst

0 1200 0.2e 4800 fmsynth-inst add-envelope constant env-inst
0.4e 0.4e  12 bpm 64ths->samples  env-inst add-delay constant env-del-inst
env-del-inst ' delay-effect-process  register-effect

 <| <c2> <-> <e2> <-> <g2> <-> <c3> <-> <e3> <-> <g3> <-> <c4> <-> <e4> <-> <g4> <-> |> constant synseq
 <| <c2> <g2> <c3> <g3> |> constant synseq0

 <|
  <d#4> <d#4> <c4> <d4> <-> <g3> <a#4> <g3>
  <d#4> <d#4> <c4> <d4> <-> <g3> <a#4> <g3>
  <d#4> <d#4> <c4> <d4> <-> <g3> <a#4> <g3>
  <d#4> <d#4> <c4> <d4> <-> <g3> <a#4> <g3>
  <f4> <f4> <d#4> <d4> <-> <a#4> <c4> <a#4>
  <f4> <f4> <d#4> <d4> <-> <g3> <g4> <d4> |> constant devil

<| <d0> |> constant drum

new-scene constant scene0
devil env-del-inst true -1 scene0 add-to-scene
drum kickdrum true -2 scene0 add-to-scene

scene0 add-scene

init-ring constant ring
: drain-ring BEGIN ring ring-count WHILE 10 pa-sleep REPEAT ;

create stream 0 ,

: main-ring
  ring orch-fill-ring drop
  pa-initialize throw
  ." Initialized" cr
  stream 0  2  0x8  sample-rate s>f  256 ring-cb  ring pa-open-default-stream throw
  ." Created stream" cr
  stream @ pa-start-stream throw
  ." Started stream" cr
  ring orch-fill-loop
  ." Draining" cr
  drain-ring
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  ;
