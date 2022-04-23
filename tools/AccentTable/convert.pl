#!/usr/bin/perl
use strict;
use warnings;

  my $closed = 1;
  my $closed2 = 1;
while(<STDIN>){
  if( $_ =~/^\"?([^\s\"]+)\"?:$/ ){
    print ");\n  }\n  break;\n  case \'".$1."\':\n  switch(y){";
    $closed = 1;
    $closed2 = 1;
  }elsif( $_ =~/^\s\s\"?([^\s\"]+)\"?:$/ ){
#print "\n".$closed . "/" . $closed2 . "\n";
    $closed = 1;
    my $d1=$1;
    if($closed2 == 1){$closed2 = 0;}else{print ");";}
    if( $d1 eq "'" ) {$d1 = "\\".$d1;}
    print "\n    case \'".$d1."\': return (";
  }elsif($_ =~/^\s+\-\s*(.+)$/ ){
    if($closed == 1){$closed = 0;}else{print ", ";}
    print "\"".$1."\"";
  }
}
