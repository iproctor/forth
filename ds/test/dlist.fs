require ../dlist.fs
require ../../test/test.fs

0 0 0 dlist-new constant head
1 head dlist-ins-after
head dlist>next@ dlist>val @ 1 =ok
head dlist>next@ dlist>prev@ dlist>val @ 0  =ok
head dlist>next@ dlist-rm-node
head dlist>next@ 0 =ok
bye
