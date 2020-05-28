require instrument.fs
require voice.fs

instrument%
  cell% field envel>inst
  cell% field envel>attack
  cell% field envel>decay
  cell% field envel>sustain
  cell% field envel>release
end-struct envel%

voice%
  cell% field envel-voice>subvoice
  cell% field envel-voice>dur
end-struct envel-voice%
: envel-voice>dur@ envel-voice>dur @ ;
: envel-voice>attack@ voice>instrument @ envel>attack @ ;
: envel-voice>decay@ voice>instrument @ envel>decay @ ;
: envel-voice>release@ voice>instrument @ envel>release @ ;
: envel-voice>sustain@ voice>instrument @ envel>sustain f@ ;

: envel-voice-in-attack? ( u envel-voice -- flag ) envel-voice>attack@ < ;
: envel-voice-scale-attack ( u envel-voice -- r ) envel-voice>attack@ s>f s>f fswap f/ ;
: envel-voice-in-decay? ( u envel-voice -- flag ) v. envel-voice>decay@ envel-voice>attack@ + < ;
: envel-voice-scale-decay ( u envel-voice -- r ) >r r@ envel-voice>attack@ - s>f r@ envel-voice>decay@ s>f f/ 1e r> envel-voice>sustain@ f- f* 1e fswap f- ;

: envel-voice-scale-adr ( u envel-voice -- r )
  2dup envel-voice-in-attack? IF envel-voice-scale-attack exit THEN
  2dup envel-voice-in-decay? IF envel-voice-scale-decay exit THEN
  nip envel-voice>sustain@ ;

: envel-voice-finished? ( u envel-voice -- flag ) envel-voice>dur @ >= ;
: envel-voice-pre-release-dur ( envel-voice -- u ) v. envel-voice>dur@ envel-voice>release@ - ;
: envel-voice-scale-at-rel ( envel-voice -- r ) v. envel-voice-pre-release-dur envel-voice-scale-adr ;
: envel-voice-in-release? ( u envel-voice -- flag ) envel-voice-pre-release-dur >= ;
: envel-voice-time-since-rel ( u envel-voice -- u ) envel-voice-pre-release-dur - ;
: envel-voice-scale-release ( u envel-voice -- r ) tuck envel-voice-time-since-rel s>f dup envel-voice>release@ s>f f/ 1e fswap f- envel-voice-scale-at-rel f* ;

: envel-voice-scale-at ( u envel-voice -- r flag )
  2dup envel-voice-finished? IF 2drop 0e true exit THEN
  2dup envel-voice-in-release? IF envel-voice-scale-release false exit THEN
  envel-voice-scale-adr false ;

: envel-gen ( t envel-voice -- n n flag ) 2dup envel-voice>subvoice @ voice>gen
  IF 2swap 2drop true exit THEN
  2swap envel-voice-scale-at v scale-samples ;

: envel-destr ( voice -- ) envel-voice>subvoice @ voice-free ;

: new-envel-voice ( dur subvoice envel -- envel-voice ) ['] envel-gen ['] envel-destr envel-voice% %alloc v. voice-init >r r@ envel-voice>subvoice ! r@ envel-voice>dur ! r> ;

: envel-add-rel ( dur inst -- dur ) envel>release @ + ;
: envel>trigger ( note dur inst -- voice ) >r r@ envel-add-rel tuck r@ envel>inst @ instrument-trigger r> new-envel-voice ;

: add-envelope ( a d s r inst -- inst ) ['] envel>trigger 1e envel% %alloc v. instrument-init >r
  r@ envel>inst !  r@ envel>release !  r@ envel>sustain f!  r@ envel>decay !  r@ envel>attack !
  r> ;
