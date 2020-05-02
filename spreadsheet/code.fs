require ../combis.fs
require ../utils.fs
require sheet.fs

\ : mk+ ( n1 "name" -- ) ['] + >r :noname r> compile, POSTPONE ; ;

: ws? ( c -- flag ) bl = ;

: step-str ( c-addr u -- c-addr u ) v char+ -1 ;
: skip-ws ( c-addr u -- c-addr u ) BEGIN
    dup WHILE
    over c@ ws? WHILE
    step-str
  AGAIN THEN THEN ;

: skip-until-ws ( c-addr u -- c-addr u ) BEGIN
    dup WHILE
    over c@ ws? 0= WHILE
    step-str
  AGAIN THEN THEN ;

: numerical? ( c -- flag ) [CHAR] 0 [CHAR] 9 within-range? ;

: parse-num ( c-addr u -- c-addr u )
  >float 0= throw
  skip-until-ws
  POSTPONE fliteral ;

1 constant err:syntax
2 constant err:eol
3 constant err:undef

: pchr ( c-addr u c -- c-addr u )
  over 0= IF err:eol throw THEN
  v over c@ <> IF err:syntax throw THEN
  step-str ;

: letter>num ( c -- u ) CASE
    dup [CHAR] A [CHAR] Z within-range? true OF [CHAR] A - ENDOF
    dup [CHAR] a [CHAR] z within-range? true OF [CHAR] a - ENDOF
    err:syntax throw
  ENDCASE ;

: parse-col ( c-addr u -- c-addr u u ) 0 >r BEGIN
    dup WHILE
    over c@ letter>num
    r> 26 * + >r
    step-str
  AGAIN >r ;

: parse-row ( c-addr u -- c-addr u u ) 0. 2swap >number 2swap 0<> IF err:syntax throw THEN ;

: parse-slice ( c-addr u -- c-addr u )
  [CHAR] [ pchr
  parse-col POSTPONE literal  parse-row POSTPONE literal
  [CHAR] : pchr
  parse-col POSTPONE literal  parse-row POSTPONE literal
  [CHAR] ] pchr
  POSTPONE grid->slice ;

s" ss:" constant func-prefix constant \func-prefix
: apply-func-prefix-here ( -- c-addr ) func-prefix \func-prefix here-append ;

: prefix-func ( c-addr u -- c-addr u )
  apply-func-prefix-here
  swap v. cmove
  here swap \func-prefix + ;

: parse-func ( c-addr u -- c-addr u )
  2dup skip-until-ws 2>r  r@ -
  prefix-func find-name dup 0= IF err:undef throw THEN
  name>int compile,  2r> ;

: parse-words ( c-addr u -- c-addr u ) BEGIN
    skip-ws  dup 0= IF exit THEN
    over c@ CASE
      dup [CHAR] [ of parse-slice ENDOF
      dup numerical? true of parse-num ENDOF
      parse-func
    ENDCASE
  AGAIN ;

