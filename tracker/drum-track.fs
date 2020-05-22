require orchestrator.fs

s" samples/KickDrum/" load-samples drop new-samples-instrument constant kickdrum
\ s" samples/SnareDrum/" load-samples drop new-samples-instrument constant snaredrum

: qdrum <c0> <-> <-> <-> ;
: qrest <-> <-> <-> <-> ;

<| qdrum qrest qdrum qrest |> constant kickseq
<| qrest qdrum qrest qdrum |> constant snareseq

new-scene constant scene0
kickseq kickdrum false 0 scene0 add-to-scene
\ snareseq snaredrum false 0 scene0 add-to-scene

scene0 add-scene

scene0 scene>slots @ list-anchor->list@ list->val@ constant kickslot
\ scene0 scene>slots @ list-anchor->list@ list->next@ list->val@ constant snareslot

init-ring constant ring
: drain-ring BEGIN ring ring-count WHILE 10 pa-sleep REPEAT ;

create stream 0 ,

: main-ring
  ring orch-fill-ring
  pa-initialize throw
  ." Initialized" cr
  stream 0  2  0x8  sample-rate s>f  256 ring-cb  ring pa-open-default-stream throw
  ." Created stream" cr
  stream @ pa-start-stream throw
  ." Started stream" cr
  ring orch-fill-loop
  drain-ring
  stream @ pa-stop-stream throw
  ." Stopped stream" cr
  stream @ pa-close-stream throw
  pa-terminate
  bye ;
