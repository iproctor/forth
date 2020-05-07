require ../combis.fs

0 value port-col
0 value port-row

24 constant cell-width

: term-esc 27 emit ;
: col-name-width ( u -- ) 1 swap BEGIN 26 / dup 0 > WHILE v 1+ 1- REPEAT drop ;
: col-name ( u  -- ) recursive  26 /mod  dup 0 > IF 1- col-name ELSE drop THEN [CHAR] A + emit ;
: left-spaces ( u -- ) col-name-width cell-width swap - 1- 2 / spaces ;
: /up ( n n -- n ) /mod swap IF 1+ THEN ;
: right-spaces ( u -- ) col-name-width cell-width swap - 1- 2 /up spaces ;
: bold-white-blue-bg ." [44;1m" ;
: col-header ( u -- ) dup left-spaces  dup col-name  right-spaces ." |" ;
: header-fmt ( -- ) term-esc bold-white-blue-bg ;
: header-end-fmt ( -- ) term-esc ." [0m" ;
: header ( -- ) header-fmt 0 0 at-xy  form nip  cell-width / 0 U+DO port-col i +  col-header LOOP  header-end-fmt ;
