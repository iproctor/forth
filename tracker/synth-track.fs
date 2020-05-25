require orchestrator.fs
require single-cycle.fs

120 to bpm

0.5e sin-sample \sin-sample new-single-cycle-instrument constant sin-inst
0.5e square-sample \square-sample new-single-cycle-instrument constant square-inst

 <| <c2> <-> <e2> <-> <g2> <-> <c3> <-> <e3> <-> <g3> <-> <c4> <-> <e4> <-> <g4> <-> |> constant synseq

new-scene constant scene0
synseq square-inst true -1 scene0 add-to-scene

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
