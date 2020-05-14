require ../../test/test.fs
require ../ring.fs

16 plan

init-ring

ring-capacity frame-count =ok
ring-capacity-ms buf-dur-ms =ok
1 2 0 ring-frame+!
1 2 0 ring-frame+!
1 ring-adv-write
ring-capacity frame-count 1- =ok
create tbuf 2 frames allot
tbuf 1 ring-read 1 =ok
tbuf left-channel sw@ 2 =ok
tbuf right-channel sw@ 4 =ok
ring-capacity frame-count =ok
frame-count 1- dup to write-ptr to read-ptr
1 2 0 ring-frame+!
2 4 1 ring-frame+!
2 ring-adv-write
ring-capacity frame-count 2 - =ok
tbuf 2 ring-read 2 =ok
ring-capacity frame-count =ok
read-ptr 1 =ok write-ptr 1 =ok

tbuf left-channel sw@ 1 =ok  tbuf right-channel sw@ 2 =ok
tbuf 1 frames + dup  left-channel sw@ 2 =ok  right-channel sw@ 4 =ok

bye
