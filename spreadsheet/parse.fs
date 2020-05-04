require ../combis.fs
require ../utils.fs
require ../ds/list.fs
require sheet.fs

0 VALUE cell-dependency-buffer
: set-dep-buffer ['] cell-dependency-buffer >body ! ;
: clear-deps 0 set-dep-buffer ;
: push-dep ( u u -- ) cell-dependency-buffer list-prepend2 set-dep-buffer ;

: ws? ( c -- flag ) bl = ;

: step-str ( c-addr u -- c-addr u ) 1 /string ;
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

: parse-number ( c-addr u -- c-addr u )
  2dup skip-until-ws 2>r r@ -
  >float 0= throw
  POSTPONE fliteral
  2r> ;

1 constant err:syntax
2 constant err:eol
3 constant err:undef

: pchr ( c-addr u c -- c-addr u )
  over 0= IF drop err:eol throw THEN
  >r over c@ r> <> IF err:syntax throw THEN
  step-str ;

: letter>num ( c -- u ) CASE
    dup [CHAR] A [CHAR] Z within-range? true OF [CHAR] A - ENDOF
    drop dup [CHAR] a [CHAR] z within-range? true OF [CHAR] a - ENDOF
    2drop -1 0
  ENDCASE 1+ ;

: parse-col ( c-addr u -- c-addr u u ) 0 >r BEGIN
    dup WHILE
    over c@ letter>num ?dup 0<> WHILE
    r> 26 * + >r
    step-str
  AGAIN THEN THEN
  r> dup 0= IF err:syntax throw THEN
  1- ;

: parse-row ( c-addr u -- u ) dup >r  0. 2swap >number
  dup r> = IF err:syntax throw THEN  2swap 0<> IF err:syntax throw THEN 1- ;

: push-slice-deps ( u u u u -- ) { c1 r1 c2 r2 } c2 1+ c1 U+DO r2 1+ r1 U+DO i j push-dep LOOP LOOP ;

: parse-slice-indices ( c-addr u -- c-addr u )
  [CHAR] [ pchr
  parse-col dup POSTPONE literal -rot
  parse-row dup POSTPONE literal -rot
  [CHAR] : pchr
  parse-col dup POSTPONE literal -rot
  parse-row dup POSTPONE literal -rot
  [CHAR] ] pchr
  2>r push-slice-deps 2r> ;

: parse-slice ( c-addr u -- c-addr u ) parse-slice-indices POSTPONE grid->slice ;

: grid->var ;

: parse-var-indices ( c-addr u -- c-addr u )
  [CHAR] { pchr
  parse-col POSTPONE literal  parse-row POSTPONE literal
  [CHAR] } pchr ;

: parse-var ( c-addr u -- c-addr u ) parse-slice-indices POSTPONE grid->var ;

s" ss:" constant \func-prefix constant func-prefix
: apply-func-prefix-here ( -- c-addr ) func-prefix \func-prefix here-append ;

: prefix-func ( c-addr u -- c-addr u )
  apply-func-prefix-here
  swap v. cmove
  here swap \func-prefix + ;

: parse-func ( c-addr u -- c-addr u )
  2dup skip-until-ws 2>r  r@ -
  prefix-func find-name dup 0= IF err:undef throw THEN
  name>int compile,  2r> ;

: <|> ( c-addr u xt xt -- c-addr u ) >r >r 2dup
  r> catch IF 2drop r> execute ELSE rdrop 2swap 2drop THEN ;

: parse-words ( c-addr u -- ) BEGIN
    skip-ws  dup 0<> WHILE
    over c@ CASE
      [CHAR] [ of parse-slice ENDOF
      [CHAR] { of parse-var ENDOF
      drop ['] parse-number ['] parse-func <|>
    0 ENDCASE
  REPEAT 2drop ;

: parse-code ( c-addr u -- xt ) step-str 2>r :noname 2r> parse-words POSTPONE ; ;

: cell-parse ( c-addr -- ) dup cell->str CASE
    2dup blank-str? true OF type:string ENDOF
    drop 2dup >float true OF over cell->val f!  type:num ENDOF
    drop over c@ [CHAR] = OF parse-code cell->val !  type:code ENDOF
    drop type:string
  0 ENDCASE
  swap cell->type ! ;
