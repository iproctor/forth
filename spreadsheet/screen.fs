require ../combis.fs
require sheet.fs
require cell_ops.fs

0 value port-col
0 value port-row
0 value cursor-col
0 value cursor-row

4 constant ruler-width
24 constant cell-width
: /up ( n n -- n ) /mod swap IF 1+ THEN ;

: port-width ( -- u ) form nip ruler-width - ;
: port-cell-width ( -- u ) port-width cell-width /up ;
: port-height ( -- u ) form drop 1- ;
: bound-port ( -- ) port-col bound-coord to port-col  port-row bound-coord to port-row ;
: inc-port-row ( u -- ) port-row + TO port-row  bound-port ;
: inc-port-col ( u -- ) port-col + TO port-col  bound-port ;

: cursor-up ( -- ) cursor-row dup IF 1- TO cursor-row ELSE drop -1 inc-port-row THEN ;
: cursor-down ( -- ) cursor-row 1+ dup port-height < IF TO cursor-row ELSE drop 1 inc-port-row THEN ;
: cursor-left ( -- ) cursor-col dup IF 1- TO cursor-col ELSE drop -1 inc-port-col THEN ;
: cursor-right ( -- ) cursor-col 1+ dup port-cell-width < IF TO cursor-col ELSE drop 1 inc-port-col THEN ;

: term-esc 27 emit ;
: end-fmt ( -- ) term-esc ." [0m" ;
: at-row ( u -- ) 0 swap at-xy ;
: available-width ( u -- u ) port-width swap - cell-width min 1- ;
: screen-cell-col ( u -- u ) cell-width / port-col + ;
: for-row ( ... xt -- )  port-width 0 U+DO ." |"  i available-width  i screen-cell-col  rot v. execute  cell-width +LOOP drop ;

: col-name-width ( u -- ) 1 swap BEGIN 26 / dup 0 > WHILE v 1+ 1- REPEAT drop ;
: col-name ( u  -- ) recursive  26 /mod  dup 0 > IF 1- col-name ELSE drop THEN [CHAR] A + emit ;
: left-spaces ( u u -- ) col-name-width -  2 /  spaces ;
: right-spaces ( u u -- ) col-name-width -  2 /up  spaces ;
: col-header ( u u -- ) 2dup left-spaces  dup col-name  right-spaces ;
: header-fmt ( -- ) term-esc ." [7;1m" ;
: scr-header ( -- ) 0 0 at-xy  ruler-width spaces  header-fmt  ['] col-header for-row  end-fmt ;

: cursor-cell-coords ( -- u u ) port-col cursor-col + port-row cursor-row + ;
: active-cell ( u u -- flag ) cursor-cell-coords d= ;
: cell-fmt ( u u -- ) term-esc  2dup active-cell IF 2drop ." [48;5;238m" ELSE
    nip 1 and IF ." [48;5;235m" ELSE ." [0m" THEN THEN ;
: row-pos ( u -- u ) port-row - 1+  at-row ;
: render-string ( c-addr u u -- ) 2dup swap - >r  min type  r> spaces ;
: render-num ( r u -- ) dup 2 - 0 f.rdp ;
: render-cell ( u u u -- ) swap  >r swap 2dup cell-fmt grid->cell r> over 0= IF spaces drop ELSE
    >r dup cell-is-string? IF cell->str r> render-string ELSE
    cell->val@ r> render-num THEN THEN ;
: render-cell-iter ( u u u -- u ) 2>r dup 2r> render-cell ;
: render-ruler ( u -- ) ruler-width u.r ;
: row ( u -- ) dup row-pos  dup render-ruler  ['] render-cell-iter for-row  drop end-fmt ;

: render scr-header port-height 0 U+DO port-row i + row LOOP ;

: edit-cur-cell ( -- ) cursor-cell-coords edit-cell ;

: handle-input ( -- ) BEGIN key CASE
    [CHAR] q OF exit ENDOF
    [CHAR] k OF cursor-up ENDOF
    [CHAR] j OF cursor-down ENDOF
    [CHAR] h OF cursor-left ENDOF
    [CHAR] l OF cursor-right ENDOF
    [CHAR] e OF edit-cur-cell ENDOF
  ENDCASE render AGAIN ;

: screen page render handle-input ;
