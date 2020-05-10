require ../combis
require cell.fs
require sheet.fs

0 value clipboard
\cell-str 1+ constant \clip-cell

: clipboard->dims ( c-addr -- u u ) ;
: clipboard->cols ( c-addr -- u ) clipboard->dims @ ;
: clipboard->rows ( c-addr -- u ) clipboard->dims cell+ @ ;
: clipboard->data ( c-addr -- c-addr ) clipboard->dims 2 cells + ;
: clipboard->cell ( u u clip -- c-addr ) >r r@ clipboard->cols * + \clip-cell * r> clipboard->data + ;
: alloc-clipboard ( u u -- c-addr ) * \clip-cell * 2 cells + dup  allocate throw  v. erase ;
: empty-clipboard ( u u -- c-addr ) 2dup alloc-clipboard dup >r clipboard->dims 2! r> ;
: coords-to-dims ( u u u u -- u u ) v swap - abs -rot - abs swap ;
: copy-to-clipboard-cell ( c-addr u u clipboard -- ) clipboard->cell \cell-str cmove ;
: copy-to-clipboard ( u u clipboard -- ) { c r clip }
  clip clipboard->cols 0 U+DO
    clip clipboard->rows 0 U+DO
      c i + r j + grid->cell dup IF cell->sz  i j clip copy-to-clipboard-cell THEN
    LOOP LOOP ;
