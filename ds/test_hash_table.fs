#! /usr/local/bin/gforth

require ../test/test.fs
require hash_fn.fs
require hash_table.fs

10 plan

17 ' hash-num ' = new-hash-table constant test-ht
test-ht hash-table-array-length @ 17 =ok
: init-test
  20 0 U+DO
    i i 3 * test-ht hash-table-insert
  LOOP ;
init-test
test-ht hash-table-size @ 20 =ok
5 test-ht hash-table-lookup ok 15 =ok

5 14 test-ht hash-table-insert
test-ht hash-table-size @ 20 =ok
5 test-ht hash-table-lookup ok 14 =ok

5 test-ht hash-table-remove
test-ht hash-table-size @ 19 =ok
5 test-ht hash-table-lookup 0 =ok 0 =ok

test-ht free-hash-table

bye
