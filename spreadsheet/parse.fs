require ../combis.fs
require ../utils.fs
require ../ds/list.fs
require val.fs
require funcs.fs
require cell.fs
require err.fs

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
  POSTPONE fliteral POSTPONE new-const-val
  2r> ;

: pchr ( c-addr u c -- c-addr u )
  over 0= IF drop err:eol throw THEN
  >r over c@ r> <> IF err:syntax throw THEN
  step-str ;

: letter>num ( c -- u )
    dup [CHAR] A [CHAR] Z within-range? IF [CHAR] A - ELSE
    dup [CHAR] a [CHAR] z within-range? IF [CHAR] a - ELSE
    drop -1 THEN THEN 1+ ;

: parse-col ( c-addr u -- c-addr u u ) 0 >r BEGIN
    dup WHILE
    over c@ letter>num ?dup 0<> WHILE
    r> 26 * + >r
    step-str
  AGAIN THEN THEN
  r> dup 0= IF err:syntax throw THEN
  1- ;

: parse-row ( c-addr u -- u ) dup >r  0. 2swap >number
  dup r> = IF err:syntax throw THEN  2swap 0<> IF err:syntax throw THEN ;

: push-slice-deps ( u u u u -- ) { c1 r1 c2 r2 } c2 1+ c1 U+DO r2 1+ r1 U+DO j i push-dep LOOP LOOP ;

: save-index ( c-addr u u -- u c-addr u ) dup POSTPONE literal -rot ;
: parse-coordinate ( c-addr u -- u u c-addr u) parse-col save-index  parse-row save-index ;
: parse-slice-indices ( c-addr u -- c-addr u )
  [CHAR] [ pchr  parse-coordinate  [CHAR] : pchr  parse-coordinate  [CHAR] ] pchr
  2>r push-slice-deps 2r> ;

: parse-slice ( c-addr u -- c-addr u ) parse-slice-indices POSTPONE new-slice-val  ;

: new-var ( u u -- ? ) 2dup new-slice-val ;

: parse-var-indices ( c-addr u -- c-addr u )
  [CHAR] { pchr  parse-coordinate  [CHAR] } pchr
  2>r push-dep 2r> ;

: parse-var ( c-addr u -- c-addr u ) parse-var-indices POSTPONE new-var ;

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

: cell-parse ( c-addr -- ) dup cell->str
  2dup blank-str? IF 2drop type:string ELSE
  2dup >float IF 2drop dup cell->val f!  type:num ELSE
  over c@ [CHAR] = = IF parse-code over cell->code !  cell-dependency-buffer over cell->deps !  clear-deps  type:code ELSE
  2drop type:string
  THEN THEN THEN
  swap cell->type c! ;
