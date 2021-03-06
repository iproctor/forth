require ../ds/list.fs
require ../ds/sort.fs
require sequence.fs
require bpm.fs

\ F - E
create note-keys CHAR a , CHAR , , CHAR o , CHAR . , CHAR e , CHAR p , CHAR u , CHAR i , CHAR f , CHAR d , CHAR g , CHAR h ,

struct
  cell% field instrument>xt
  cell% field instrument>gain
end-struct instrument%
: instrument>xt@ ( instrument -- xt ) instrument>xt @ ;
: instrument>gain@ ( instrument -- r ) instrument>gain f@ ;
: instrument-trigger ( note-index dur instrument -- voice ) dup instrument>xt@ execute ;
: instrument-play-trigger ( trigger instrument -- voice ) >r v. trigger-note-index trigger-dur@ bpm 64ths->samples r> instrument-trigger ;
: instrument-init ( xt r:gain instrument -- ) >r r@ instrument>xt ! r> instrument>gain f! ;

