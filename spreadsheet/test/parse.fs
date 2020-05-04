#! /usr/local/bin/gforth

require ../../test/test.fs
require ../parse.fs

10 plan

: str-empty s" " str= ok ;

s"  a " skip-ws s" a " str= ok
s" a a" skip-until-ws s"  a" str= ok

CHAR 1 numerical? ok
: t [ s" 1.2" parse-number str-empty ] ; t 1.2e0 f= ok

s" ab" CHAR a pchr s" b" str= ok
: perr [CHAR] a pchr ;
s" q" ' perr catch err:syntax =ok 2drop

CHAR b letter>num 2 =ok
CHAR B letter>num 2 =ok
CHAR 1 letter>num 0 =ok

s" AB1" parse-col 27 =ok s" 1" str= ok
s" " ' parse-col catch err:syntax =ok 2drop
s" 1" ' parse-col catch err:syntax =ok 2drop
s" 12" parse-row 12 =ok str-empty

s" A12" ' parse-row catch err:syntax =ok 2drop
s" " ' parse-row catch err:syntax =ok 2drop

: tsi [ s" [A1:H33]" parse-slice-indices str-empty ] ;
tsi 33 =ok 7 =ok 1 =ok 0 =ok

: tvi [ s" {H33}" parse-var-indices str-empty ] ;
tvi 33 =ok  7 =ok

apply-func-prefix-here here \func-prefix + =ok
s" ff" prefix-func s" ss:ff" str= ok

: ss:tst 123 ;
: tpf [ s" tst" parse-func str-empty ] ;
tpf 123 =ok

: ss:2* f2* ;
: tpw [ s" 2.1 2*" parse-words ] ;
tpw 4.2e f= ok

: tpc s" =2.1 2*" parse-code ;
tpc execute 4.2e f= ok

bye
