require ../combis.fs
require cell.fs
require sheet.fs
require cell_ops.fs

0 value clipboard
\cell-str 1+ constant \clip-cell

: coords-to-dims ( u u u u -- u u ) v swap - abs -rot - abs swap  v 1+ 1+ ;

: clipboard->dims ( -- u u ) clipboard ;
: clipboard->cols ( -- u ) clipboard->dims cell+ @ ;
: clipboard->rows ( -- u ) clipboard->dims @ ;
: clipboard->data ( -- c-addr ) clipboard->dims 2 cells + ;
: clipboard->cell ( u u -- c-addr ) clipboard->cols * + \clip-cell *  clipboard->data + ;
: clipboard->str ( u u -- c-addr u ) clipboard->cell v. c@ char+ swap ;

: free-clipboard ( -- ) clipboard IF clipboard free throw THEN 0 to clipboard ;
: alloc-clipboard ( u u -- c-addr ) * \clip-cell * 2 cells + dup  allocate throw  dup rot erase  to clipboard ;
: new-clipboard ( u u -- ) free-clipboard  2dup alloc-clipboard  clipboard->dims 2! ;

: copy-to-clipboard-cell ( c-addr u u -- ) clipboard->cell \cell-str cmove ;
: copy-to-clipboard ( u u -- ) { c r }
  clipboard->cols 0 U+DO
    clipboard->rows 0 U+DO
      c j + r i + grid->cell dup IF cell->sz  j i copy-to-clipboard-cell THEN
    LOOP LOOP ;

: paste ( u u -- ) { c r }
  clipboard 0= IF exit THEN
  clipboard->cols 0 U+DO
    clipboard->rows 0 U+DO
      j i clipboard->str  c j + r i + grid->cell! str>cell
      c j + r i + cell-done-edit
    LOOP LOOP ;

: copy ( u u u u -- ) 2>r 2dup 2r> coords-to-dims new-clipboard copy-to-clipboard ;
