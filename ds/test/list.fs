require ../../test/test.fs
require ../list.fs

10 plan

1 0 list-prepend
2 swap list-prepend
dup list->next @ list->val @ 1 =ok
free-list

: pp list-prepend2 ;
: test-list 1 2 2 4 3 6 4 8 0 pp pp pp pp ;
test-list dup dup list-rm-next list->next @ list->val 2@ 6 =ok 3 =ok free-list

1 2 ' 2<> test-list list-drop-until2 dup list->val 2@ 4 =ok 2 =ok free-list

1 1 0 list-filter-out2 0 =ok
2 4 test-list list-filter-out2 dup list->next @ list->val 2@ 6 =ok 3 =ok free-list

bye
