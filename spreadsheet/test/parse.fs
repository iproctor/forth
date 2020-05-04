#! /usr/local/bin/gforth

require ../../test/test.fs
require ../parse.fs

39 plan

: str-empty s" " str= ok ;

s"  a " skip-ws s" a " str= ok
s" a a" skip-until-ws s"  a" str= ok

." # parse-number" cr
CHAR 1 numerical? ok
: t [ s" 1.2" parse-number str-empty ] ; t 1.2e0 f= ok

." # pchr" cr
s" ab" CHAR a pchr s" b" str= ok
: perr [CHAR] a pchr ;
s" q" ' perr catch err:syntax =ok 2drop

CHAR b letter>num 2 =ok
CHAR B letter>num 2 =ok
CHAR 1 letter>num 0 =ok

." # parse-col" cr
s" AB1" parse-col 27 =ok s" 1" str= ok
s" " ' parse-col catch err:syntax =ok 2drop
s" 1" ' parse-col catch err:syntax =ok 2drop

." # parse-row" cr
s" 12" parse-row 11 =ok str-empty
s" A12" ' parse-row catch err:syntax =ok 2drop
s" " ' parse-row catch err:syntax =ok 2drop

." # parse-slice-indices" cr
: tsi [ s" [A1:H33]" parse-slice-indices str-empty ] ;
tsi 32 =ok 7 =ok 0 =ok 0 =ok

." # parse-var-indices" cr
: tvi [ s" {H33}" parse-var-indices str-empty ] ;
tvi 32 =ok  7 =ok

." # prefix-func" cr
apply-func-prefix-here here \func-prefix + =ok
s" ff" prefix-func s" ss:ff" str= ok

." # parse-func" cr
: ss:tst 123 ;
: tpf [ s" tst" parse-func str-empty ] ;
tpf 123 =ok

." # parse-words" cr
: ss:2* f2* ;
: tpw [ s" 2.1 2*" parse-words ] ;
tpw 4.2e f= ok

." # parse-code" cr
: tpc s" =2.1 2*" parse-code ;
tpc execute 4.2e f= ok

." # cell-parse" cr
allot-block constant test-blk
0 0 test-blk block->cell constant test-cell
s" " test-cell str>cell
test-cell cell-parse
test-cell cell->type c@ type:string =ok

s" goober" test-cell str>cell
test-cell cell-parse
test-cell cell->str s" goober" str= ok
test-cell cell->type c@ type:string =ok

s" 1.2" test-cell str>cell
test-cell cell-parse
test-cell cell->type c@ type:num =ok
test-cell cell->val f@ 1.2e f= ok

s" = 1.2 2*" test-cell str>cell
test-cell cell-parse
test-cell cell->type c@ type:code =ok
test-cell cell->val @ execute 2.4e f= ok

bye
