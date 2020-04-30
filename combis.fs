: v postpone >r ' compile, postpone r> ; immediate
: v. postpone dup postpone v ; immediate

: dip ( w xt -- w )
  swap >r
  execute
  r> ;

: 2dip ( w w xt -- w w )
  -rot 2>r
  execute
  2r> ;

: keep ( w xt -- w )
  over >r
  execute
  r> ;

: bi ( w xt xt -- ) { x p q }
  x p execute
  x q execute ;

: tri ( w xt xt xt -- ) { x p q r }
  x p execute
  x q execute
  x r execute ;

: quad ( w xt xt xt xt -- ) { x p q r s }
  x p execute
  x q execute
  x r execute
  x s execute ;

: bi* ( w w xt xt -- ) { x y p q }
  x p execute
  y q execute ;

: tri* ( w w xt xt -- ) { x y z p q r }
  x p execute
  y q execute
  z r execute ;

: bi@ ( w w xt -- ) { x y p }
  x p execute
  y p execute ;

: tri@ ( w w w xt -- ) { x y z p }
  x p execute
  y p execute
  z p execute ;

: quad@ ( w w w w xt -- ) { w x y z p }
  w p execute
  x p execute
  y p execute
  z p execute ;

: when ( w xt -- ) { x p }
  x IF x p execute THEN ;
