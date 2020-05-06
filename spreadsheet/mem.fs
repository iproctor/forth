0 VALUE val-mem-list
: set-val-mem-list ['] val-mem-list >body ! ;
: free-val-mem val-mem-list free-mem-list  0 set-val-mem-list ;
: alloc-val ( u -- c-addr ) val-mem-list alloc-push  set-val-mem-list ;
