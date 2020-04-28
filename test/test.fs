\ TAP output

Variable test# 
0 test# !

: plan ( n -- )
	." 1.." . cr
;

\ Compile a word that outputs inline text
\ : :comment
\ 	header (:noname)
\ ;

\ immediate
: no-plan ( -- )

;

: ok ( f -- )
	0= if ." not " then
	." ok "
	test# dup @ 1+ dup . swap !
	cr
;

: pass ( -- )
	true ok
;

: fail ( -- )
	false ok
;

: =ok ( n n -- )
	= ok
;

\ This is an idiom for running a bunch of tests in a stack frame.
\ I've no idea whether this is a nice thing to do.

Variable frame 
0 frame !

: t{
	frame @
	sp@ frame !
;

: t=
	sp@ frame @ =ok
;

: t}
	t=
	frame !
;
