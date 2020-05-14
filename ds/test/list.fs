#! /usr/local/bin/gforth

require ../../test/test.fs
require ../list.fs

13 plan

1 0 list-prepend
2 swap list-prepend
dup list->next @ list->val @ 1 =ok
list-free

: pp list-prepend2 ;
: test-list 1 2 2 4 3 6 4 8 0 pp pp pp pp ;
test-list dup dup list-rm-next list->next @ list->val 2@ 6 =ok 3 =ok list-free

1 2 ' 2<> test-list list-drop-until2 dup list->val 2@ 4 =ok 2 =ok list-free

1 1 0 list-filter-out2 0 =ok
2 4 test-list list-filter-out2 dup list->next @ list->val 2@ 6 =ok 3 =ok list-free

." #anchors" cr
new-list-anchor constant anc
3 anc list-anchor-append
2 anc list-anchor-prepend
4 anc list-anchor-append
1 anc list-anchor-prepend
anc list-anchor->list @
dup list->val @ 1 =ok list->next @
dup list->val @ 2 =ok list->next @
dup list->val @ 3 =ok list->next @
dup list->val @ 4 =ok list->next @ 0 =ok

bye
