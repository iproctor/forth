\ Assoc list
struct
  cell% field assoc-node-next
  cell% field assoc-node-key
  cell% field assoc-node-val
end-struct assoc-node%

: new-assoc-node ( assoc-node w w -- assoc-node ) { node key val }
  assoc-node% %allocate throw
  node over assoc-node-next !
  key over assoc-node-key !
  val over assoc-node-val ! ;

: free-assoc-list ( assoc-node -- )
  BEGIN
    dup WHILE
    dup assoc-node-next @
    swap free throw
  REPEAT
  drop ;

: assoc-from-array ( a-addr u -- assoc-node ) { array n }
  0  n 0 U+DO
    array i cells + @  array i 1+ cells + @  new-assoc-node
  2 +LOOP ;

: assoc-node-test ( assoc-node w xt -- flag )
  rot assoc-node-key @ -rot  execute ;

: assoc-lookup ( w xt assoc-node -- val flag ) { pred-arg pred assoc-node }
  assoc-node
  BEGIN
    dup WHILE
    dup pred-arg pred assoc-node-test IF
      assoc-node-val @  true  exit
    THEN
    assoc-node-next @
  REPEAT
  drop 0 false ;

: assoc-for-each ( xt assoc-node .. )
  \ xt wants only k and v on the stack at execute time
  2>r
  BEGIN
    r@ WHILE
      r@ assoc-node-key @
      2r@ assoc-node-val @
      swap execute
      r> assoc-node-next @ >r
  REPEAT
  2rdrop ;

: show-pair ( w w .. )
  swap ." ( " . . ." ) " ;

: assoc-show ( assoc-node .. )
  ['] show-pair swap assoc-for-each ;

: assoc-remove ( w xt assoc-node -- assoc-node flag ) { pred-arg pred assoc-node }
  assoc-node 0= IF 0 false exit THEN
  assoc-node pred-arg pred assoc-node-test IF
    assoc-node assoc-node-next @  true  exit THEN
  assoc-node
  assoc-node assoc-node-next
  BEGIN ( prev cur )
    dup WHILE
    dup pred-arg pred assoc-node-test IF
      dup assoc-node-next @ -rot
      free throw
      assoc-node-next !
      assoc-node true exit
    THEN
    nip dup assoc-node-next @
  REPEAT
  2drop assoc-node false ;

: assoc-insert ( assoc-node w w xt -- assoc-node flag ) { assoc-node key val pred }
  assoc-node 0= IF 0 key val new-assoc-node true exit THEN
  0 assoc-node \ prev and cur
  BEGIN
    dup WHILE
    dup key pred assoc-node-test IF
      assoc-node-val val swap !
      drop \ drop prev
      assoc-node false exit
    THEN
    nip dup \ nip prev
    assoc-node-next @
  REPEAT
  drop \ drop cur
  0 key val new-assoc-node
  swap assoc-node-next !
  assoc-node true ;
