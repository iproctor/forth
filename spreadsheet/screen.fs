require ../combis.fs
require sheet.fs

0 value port-col
0 value port-row
0 value cursor-col
0 value cursor-row

: port-width ( -- u ) form nip ;
: port-height ( -- u ) form drop 1- ;
: bound-port ( -- ) port-col bound-coord to port-col  port-row bound-coord to port-row ;
: inc-port-row ( u -- ) port-row + TO port-row  bound-port ;
: inc-port-col ( u -- ) port-col + TO port-col  bound-port ;

: cursor-up ( -- ) cursor-row dup IF 1- TO cursor-row ELSE drop -1 inc-port-row THEN ;
: cursor-down ( -- ) cursor-row 1+ dup port-height < IF TO cursor-row ELSE drop 1 inc-port-row THEN ;
: cursor-left ( -- ) cursor-col dup IF 1- TO cursor-col ELSE drop -1 inc-port-col THEN ;
: cursor-right ( -- ) cursor-col 1+ dup port-width < IF TO cursor-col ELSE drop 1 inc-port-col THEN ;

24 constant cell-width

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

: active-cell ( u u -- flag ) port-row - cursor-row =  swap port-col - cursor-col = and ;
: cell-fmt ( u u -- ) term-esc  2dup active-cell IF 2drop ." [48;5;238m" ELSE
    nip 1 and IF ." [48;5;235m" ELSE ." [0m" THEN THEN ;
: row-pos ( u -- u ) port-row - 1+  at-row ;
: render-string ( c-addr u u -- ) 2dup swap - >r  min type  r> spaces ;
: render-num ( r u -- ) dup 2 - 0 f.rdp ;
: render-cell ( u u u -- ) swap  >r swap 2dup cell-fmt grid->cell r> over 0= IF spaces drop ELSE
    >r dup cell-is-string? IF cell->str r> render-string ELSE
    cell->val@ r> render-num THEN THEN ;
: render-cell-iter ( u u u -- u ) 2>r dup 2r> render-cell ;
: row ( u -- ) dup row-pos  ['] render-cell-iter for-row  drop end-fmt ;

: render header port-height 0 U+DO port-row i + row LOOP ;

: handle-input ( -- ) BEGIN key CASE
    [CHAR] q OF exit ENDOF
    [CHAR] k OF cursor-up ENDOF
    [CHAR] j OF cursor-down ENDOF
    [CHAR] h OF cursor-left ENDOF
    [CHAR] l OF cursor-right ENDOF
  ENDCASE render AGAIN ;

: screen page render handle-input ;
