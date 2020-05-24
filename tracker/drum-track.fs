require orchestrator.fs
require sample_instrument.fs
require delay.fs

120 to bpm

0  s" samples/KickDrum/" load-samples drop 0.3e new-samples-instrument constant kickdrum
0  s" samples/SnareDrum/" load-samples drop 0.3e new-samples-instrument constant snaredrum
3 4 note-to-note-index s" samples/SynPiano/" load-samples drop 0.3e new-samples-instrument constant synpiano
0.3e 0.7e  24 bpm 64ths->samples  synpiano add-delay constant synpianodelay
synpianodelay  ' delay-effect-process  register-effect

: qdrum <d0> <-> <-> <-> ;
: qdrum2 <-> <-> <d0> <-> ;
: qrest <-> <-> <-> <-> ;

<| qdrum qdrum2 qdrum2 qdrum |> constant kickseq
\ <| qdrum qrest qdrum qrest |> constant kickseq
<| qrest qdrum qrest qdrum |> constant snareseq

: qn1 <e5> <-> <-> <-> ;
: qn2 <a5> <-> <-> <-> ;
: qn3 <-> <-> <a5> <-> ;
: qn4 <-> <e5> <-> <e5> ;
 <| qn3 qrest qn1 qrest  qn3 qn4 qn2 qrest |> constant synseq
\ <| qn1 qrest qrest qrest qrest qrest qrest qrest |> constant synseq

new-scene constant scene0
kickseq kickdrum true 0 scene0 add-to-scene
snareseq snaredrum true 0 scene0 add-to-scene
synseq synpianodelay true 0 scene0 add-to-scene

scene0 add-scene

scene0 scene>slots @ list-anchor->list@ list->val@ constant kickslot
\ scene0 scene>slots @ list-anchor->list@ list->next@ list->val@ constant snareslot

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
