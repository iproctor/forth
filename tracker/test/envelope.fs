require ../envelope.fs
require ../../test/test.fs

20 plan

4 8 0.5e 10 0 add-envelope constant ei
envel-voice% %alloc constant ev
ei ev voice>instrument !
25 ev envel-voice>dur !


ev envel-voice-pre-release-dur 15 =ok
0 ev envel-voice-finished? 0= ok
26 ev envel-voice-finished? ok
16 ev envel-voice-in-release? ok
20 ev envel-voice-scale-release 0.25e f= ok
0 ev envel-voice-in-attack? ok
0 ev envel-voice-scale-attack 0e f= ok
2 ev envel-voice-scale-attack 0.5e f= ok
4 ev envel-voice-in-decay? ok
4 ev envel-voice-scale-decay 1e f= ok
8 ev envel-voice-scale-decay 0.75e f= ok
12 ev envel-voice-in-decay? 0= ok
12 ev envel-voice-scale-at 0.5e f= ok 0= ok
0 ev envel-voice-scale-at 0e f= ok 0= ok
4 ev envel-voice-scale-at 1e f= ok 0= ok
20 ev envel-voice-scale-at 0.25e f= ok 0= ok
bye
