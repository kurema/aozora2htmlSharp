#!/usr/bin/perl
use strict;
use warnings;
use utf8;

while(<STDIN>){
  if( $_ =~ /^:([^:]+):\s+\"([^\"]+)\"/){
    print "case \"$1\": return \"$2\";\n";
  }
}
