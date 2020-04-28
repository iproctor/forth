\ Hash functions

: hash-num ( n -- n )
  dup 30 rshift xor 0xbf58476d1ce4e5b9 *
  dup 27 rshift xor 0x94d049bb133111eb *
  dup 31 rshift xor abs ;

: hash-string ( c-addr u -- n ) { str len }
  0 0
  len 0 U+DO
    str i chars + c@  +  65521 mod
    swap over +  65521 mod  swap
  LOOP
  swap 16 lshift or abs ;
