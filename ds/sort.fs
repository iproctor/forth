require ../combis.fs
require ../utils.fs

: sorter: ( xt xt "name" -- ) { cmp swp } : ]] 1 U+DO
  i BEGIN
    dup 0 > WHILE
    >r r@ r@ 1- [[ cmp compile, ]] 0 < r> swap WHILE
    >r r@ r@ 1- [[ swp compile, ]] r>
    1-
  AGAIN THEN THEN drop LOOP [[ ;

: u-arr-cmp ( u-array u u -- n ) 2elem@ - ;
: u-arr-cmp-d ( u-array u u -- u-array n ) dup3rd u-arr-cmp ;
: swp-vals ( u u ) 2dup v @ @ swap  v swap  swap !  swap ! ;
: u-arr-swp ( u-array u u -- ) v over cells + >r cells + r> swp-vals ;
: u-arr-swp-d ( u-array u u -- u-array ) dup3rd u-arr-swp ;

' u-arr-cmp-d ' u-arr-swp-d sorter: num-sort drop ;
