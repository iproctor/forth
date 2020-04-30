\ Hash Table
\ Implements a hash table on cell arrays storing cells

require assoc_list.fs
require ../combis.fs

struct
  cell% field hash-table-array
  cell% field hash-table-array-length
  cell% field hash-table-size
  cell% field hash-table-hash
  cell% field hash-table-eq
end-struct hash-table%

: hash-table-over-threshold ( hash-table -- flag )
  dup hash-table-size @ s>f
  hash-table-array-length @ s>f
  f/ 0.8e f>= ;

: hash-table-raw-insert ( a-addr u xt xt w w -- flag )
  { array array-len hash eq key val }
  array  key hash execute  array-len mod  cells +
  dup @ key val eq assoc-insert
  ( assoc-list-addr new-node flag )
  -rot swap ! ;

: hash-table-new-zeroed-array ( n -- a-addr )
  dup cells allocate throw
  dup rot cells erase ;

\ n should be prime
: new-hash-table ( n xt xt -- hash-table )
  hash-table% %allocate throw >r
  0 r@ hash-table-size !
  r@ hash-table-eq !
  r@ hash-table-hash !
  dup hash-table-new-zeroed-array
  r@ hash-table-array !
  r@ hash-table-array-length !
  r> ;

: hash-table-for-each ( xt hash-table -- )
  { xt ht }
  ht hash-table-array-length @ 0 U+DO
    ht hash-table-array @ i cells + @
    xt swap assoc-for-each
  LOOP ;

: hash-table-show ( hash-table -- )
  ['] show-pair swap hash-table-for-each ;

: hash-table-iter-insert ( a-addr u xt xt w w -- a-addr u xt xt )
  { array array-len hash eq key val }
  array array-len hash eq key val hash-table-raw-insert drop
  array array-len hash eq ;

: hash-table-maybe-resize ( hash-table -- ) { ht }
  ht hash-table-over-threshold 0= IF exit THEN
  ht hash-table-array-length @ 2 *
  dup hash-table-new-zeroed-array swap
  ht hash-table-hash @
  ht hash-table-eq @
  ['] hash-table-iter-insert ht hash-table-for-each
  2drop \ drop hash and eq
  ht hash-table-array-length !
  ht hash-table-array ! ;

: hash-table-explode ( hash-table -- w n xt xt )
  ['] hash-table-array
  ['] hash-table-array-length
  ['] hash-table-hash
  ['] hash-table-eq
  quad
  ['] @ quad@
  ;

: hash-table-insert ( w w hash-table -- )
  { key val ht }
  ht hash-table-explode
  key val
  hash-table-raw-insert IF
    1 ht hash-table-size +!
  THEN
  ht hash-table-maybe-resize ;

: hash-table-key-slot ( w hash-table -- a-addr ) { key ht }
  ht hash-table-array @
  key ht hash-table-hash @ execute
  ht hash-table-array-length @ mod cells + ;

: hash-table-remove ( w hash-table -- ) { key ht }
  key ht hash-table-key-slot >r
  key  ht hash-table-eq @  r@ @  assoc-remove IF
    -1 ht hash-table-size +! THEN
  r> ! ;

: hash-table-lookup ( w hash-table -- w flag ) { key ht }
  key
  ht hash-table-eq @
  key ht hash-table-key-slot @
  assoc-lookup ;

: free-hash-table ( hash-table -- )
  dup hash-table-array @
  over hash-table-array-length @ 0 U+DO
    dup i cells + @ free-assoc-list
  LOOP
  free throw
  free throw ;
