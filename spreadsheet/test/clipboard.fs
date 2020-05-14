#! /usr/local/bin/gforth

require ../../test/test.fs
require ../clipboard.fs

: str-empty s" " str= ok ;

11 plan

1 2 3 6 coords-to-dims 3 5 d= ok

2 3 new-clipboard
clipboard->dims 2@ 2 3 d= ok
clipboard->rows 3 =ok
clipboard->cols 2 =ok

s" test" 0 0 grid->cell! str>cell
s" test2" 1 1 grid->cell! str>cell
0 0 grid->cell cell->sz  0 0 copy-to-clipboard-cell
clipboard->data char+ clipboard->data c@ s" test" str= ok
0 0 clipboard->str s" test" str= ok

0 0 copy-to-clipboard
0 0 clipboard->str s" test" str= ok
0 1 clipboard->str str-empty
1 1 clipboard->str s" test2" str= ok

5 5 paste
5 5 grid->cell cell->str s" test" str= ok
6 6 grid->cell cell->str s" test2" str= ok

bye
