require ../sort.fs
require ../../test/test.fs

8 plan

create test-arr 4 , 2 , 3 , 1 ,

test-arr 1 2 u-arr-cmp -1 =ok
test-arr 0 1 u-arr-cmp 2 =ok
test-arr 0 1 u-arr-swp
test-arr @ 2 =ok
test-arr cell+ @ 4 =ok
test-arr 0 1 u-arr-swp
test-arr 4 num-sort
test-arr @ 1 =ok
test-arr 1 cells + @ 2 =ok
test-arr 2 cells + @ 3 =ok
test-arr 3 cells + @ 4 =ok
bye
