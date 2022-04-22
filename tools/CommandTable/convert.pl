#!/usr/bin/perl
use strict;
use warnings;

while(<STDIN>){
  if( $_ =~/^(.+):$/ ){
    print "};\ncase \"".$1."\":\nreturn new string[]{";
  }elsif($_ =~/^\-\s*(.+)$/ ){
    print "\"".$1."\", ";
  }
}
