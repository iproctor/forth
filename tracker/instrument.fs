require ../ds/list.fs
require ../ds/sort.fs
require sequence.fs

\ F - E
create note-keys CHAR a , CHAR , , CHAR o , CHAR . , CHAR e , CHAR p , CHAR u , CHAR i , CHAR f , CHAR d , CHAR g , CHAR h ,

: instrument>xt ( instrument -- xt ) @ ;
: instrument>data ( instrument -- w ) cell+ @ ;
: instrument>gain ( instrument -- r ) 2 cells + f@ ;
: instrument>trigger ( note oct instrument -- voice ) dup instrument>xt execute ;
: instrument-play-trigger ( trigger instrument -- voice ) >r trigger-notes 2@ r> instrument>trigger ;

