#! /usr/local/bin/gforth

require ../../test/test.fs
require ../parse.fs

10 plan

s"  a " skip-ws s" a " str= ok
s" a a" skip-until-ws s"  a" str= ok

CHAR 1 numerical? ok
: t [ s" 1.2" parse-number ] ; t 1.2e0 f= ok

s" ab" CHAR a pchr s" b" str= ok
: perr [CHAR] a pchr ;
s" q" ' perr catch err:syntax =ok 2drop

CHAR b letter>num 2 =ok
CHAR B letter>num 2 =ok
CHAR 1 letter>num 0 =ok

s" AB1" parse-col 27 =ok s" 1" str= ok
s" " ' parse-col catch err:syntax =ok 2drop
s" 1" ' parse-col catch err:syntax =ok 2drop
s" 12" parse-row 12 =ok s" " str= ok

s" A12" ' parse-row catch err:syntax =ok 2drop
s" " ' parse-row catch err:syntax =ok 2drop


bye
