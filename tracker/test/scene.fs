require ../scene.fs
require ../../test/test.fs

10 plan

: qnote <c3> <-> <-> <-> ;
: qrest <-> <-> <-> <-> ;

<| qnote qrest qnote qrest |> constant testseq

new-scene constant scene0
testseq 0 false 0 scene0 add-to-scene

scene0 scene>slots @ list-anchor->list@ list->val@ constant testslot

testslot scn-slot-scn-steps 16 =ok
testslot scn-slot-step-dur 4 =ok
testslot scn-slot-seq-dur 64 =ok
testslot scn-slot-dur 64 =ok
4 testslot scn-step-from-64ths ok 1 =ok
4 testslot adjust-offset-for-loop 0 =ok 4 =ok
0 testslot scn-slot-nth-trig dup trigger-type @ trigger:note =ok
." # testing notes" cr
trigger-notes 2@ 3 =ok 0 =ok
1 testslot scn-slot-nth-trig trigger-type @ trigger:rest =ok
8 testslot scn-slot-nth-trig trigger-type @ trigger:note =ok

