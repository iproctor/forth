require ../combis.fs
\ : mk+ ( n1 "name" -- ) ['] + >r :noname r> compile, POSTPONE ; ;

32 constant spacechar
: ws? ( c -- flag ) spacechar = ;

: step-str ( c-addr u -- c-addr u ) v char+ -1 ;
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

: numerical? ( c -- flag ) [CHAR] 0 over v >= [CHAR] 9 <= and ;

: parse-num ( c-addr u -- c-addr u )
  >float 0= throw
  skip-until-ws
  POSTPONE fliteral ;

: parse-slice ;

: parse-func ;

: parse-words ( c-addr u -- c-addr u ) BEGIN
    skip-ws
    dup 0= IF exit THEN
    over c@ CASE
      dup [CHAR] [ of parse-slice ENDOF
      dup numerical? true of parse-num ENDOF
      parse-func
    ENDCASE
  AGAIN ;

