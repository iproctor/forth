#! /usr/local/bin/gforth

require ../../test/test.fs
require ../assoc_list.fs

10 plan

create test-list-1 1 , 2 , 4 , 8 , 9 , 18 ,
test-list-1 6 assoc-from-array constant test-assoc-1

1 ' = test-assoc-1 assoc-lookup ok 2 =ok
4 ' = test-assoc-1 assoc-lookup ok 8 =ok
test-assoc-1 10 20 ' = assoc-insert ok drop
10 ' = test-assoc-1 assoc-lookup ok 20 =ok
test-assoc-1 9 19 ' = assoc-insert 0= ok drop
9 ' = test-assoc-1 assoc-lookup ok 19 =ok

bye
