require ../combis.fs
require sheet.fs

0 value port-col
0 value port-row

24 constant cell-width

: port-width ( -- u ) form nip ;
: term-esc 27 emit ;
: end-fmt ( -- ) term-esc ." [0m" ;
: /up ( n n -- n ) /mod swap IF 1+ THEN ;
: at-row ( u -- ) 0 swap at-xy ;
: available-width ( u -- u ) port-width swap - cell-width min 1- ;
: for-row ( ... xt -- )  port-width 0 U+DO i available-width  i cell-width /  rot v. execute ." |"  cell-width +LOOP drop ;

: col-name-width ( u -- ) 1 swap BEGIN 26 / dup 0 > WHILE v 1+ 1- REPEAT drop ;
: col-name ( u  -- ) recursive  26 /mod  dup 0 > IF 1- col-name ELSE drop THEN [CHAR] A + emit ;
: left-spaces ( u u -- ) col-name-width -  2 /  spaces ;
: right-spaces ( u u -- ) col-name-width -  2 /up  spaces ;
: col-header ( u u -- ) 2dup left-spaces  dup col-name  right-spaces ;
: header-fmt ( -- ) term-esc ." [7;1m" ;
: header ( -- ) header-fmt  0 at-row  ['] col-header for-row  end-fmt ;

: row-fmt ( u -- ) term-esc  1 and IF ." [48;5;235m" ELSE ." [0m" THEN ;
: row-pos ( u -- u ) port-row - 1+  at-row ;
: render-string ( c-addr u u -- ) 2dup swap - >r  min type  r> spaces ;
: render-num ( r u -- ) dup 2 - 0 f.rdp ;
: render-cell ( u u u -- ) swap  v grid->cell  over 0= IF spaces drop ELSE
    >r dup cell-is-string? IF cell->str r> render-string ELSE
    cell->val@ r> render-num THEN THEN ;
: render-cell-iter ( u u u -- u ) 2>r dup 2r> render-cell ;
: row ( u -- ) dup row-fmt  dup row-pos  ['] render-cell-iter for-row  end-fmt ;

: render page header form drop 1- 0 U+DO i row LOOP ;
