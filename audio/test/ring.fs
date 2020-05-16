require ../../test/test.fs
require ../ring.fs

19 plan

init-ring constant ring

ring ring-capacity frame-count =ok
ring ring-capacity-ms buf-dur-ms =ok
1 2 0 ring ring-frame+!
1 2 0 ring ring-frame+!
1 ring ring-adv-write
ring ring>read-ptr@ 0 =ok
ring read-after-write? 0= ok
ring ring-capacity frame-count 1- =ok
ring ring-at-read ring ring>data =ok
create tbuf 2 frames allot
tbuf 1 ring ring-read 1 =ok
tbuf left-channel sw@ 2 =ok
tbuf right-channel sw@ 4 =ok
ring ring-capacity frame-count =ok
frame-count 1- dup ring ring>read-ptr ! ring ring>write-ptr !
1 2 0 ring ring-frame+!
2 4 1 ring ring-frame+!
2 ring ring-adv-write
ring ring-capacity frame-count 2 - =ok
tbuf 2 ring ring-read 2 =ok
ring ring-capacity frame-count =ok
ring ring>read-ptr@ 1 =ok ring ring>write-ptr@ 1 =ok

tbuf left-channel sw@ 1 =ok  tbuf right-channel sw@ 2 =ok
tbuf 1 frames + dup  left-channel sw@ 2 =ok  right-channel sw@ 4 =ok

bye
