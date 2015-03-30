import numpy as np
import json
import time 
import matplotlib.pyplot as pl 
from scipy.sparse import coo_matrix, csc_matrix



def getHoursCompleted( row ): 
  ts = time.strptime( row.split( "." )[0], "%Y-%m-%dT%H:%M:%S" )
  return ( ts.tm_hour + ts.tm_min / 60.0 + ts.tm_sec / ( 60.0 * 60.0 ) ) / 24.0
  
  

path = "/home/nt/crf_sgd/data/twor.2009_annotated/"

timestamps = json.load( open( path + "output_ed/timestamps.json" ) )
labels = json.load( open( path + "output_ed/labels.json" ) )
metadata = json.load( open( path + "catalogue/label_keystore.json" ) )
metadata = metadata["indexToVal"]

for i in np.arange( len ( metadata ) ): 
  metadata[i] = metadata[str(i)]
  del metadata[str(i)]


N = len( timestamps ) 

secondsInAnHour = 60.0 * 60.0
secondsInADay = 24 * secondsInAnHour

rscale = 10.0


rows = []
cols = []
vals = []


for ii, ( tt, yy ) in enumerate( zip( timestamps, labels ) ): 
  for y in yy: 
    rows.append( ii ) 
    cols.append( y )
    vals.append( 1.0 )

X = coo_matrix( ( vals, ( rows, cols ) ), shape = ( len( timestamps ), len( metadata ) ) )
X = csc_matrix( X )

X[0, :] = 0
X[-1, :] = 0

for target, tn in metadata.iteritems():
  print target, tn 
  
  x = []
  
  times = X[:, target].todense().reshape( -1 )
  diff = np.diff( times )
  
  pos = np.c_[
    np.asarray( np.where( diff > 0 )[1].tolist()[0] ) + 3, 
    np.asarray( np.where( diff < 0 )[1].tolist()[0] ) - 3
  ]
  
  print " ", len( pos ), "events"
  
  for rr in pos: 
    start = getHoursCompleted( timestamps[rr[0]] ) * secondsInADay
    stop = getHoursCompleted( timestamps[rr[1]] ) * secondsInADay
    
    print "    %7.4f %7.4f %9d %9d %9d" % ( 
      start / secondsInAnHour, 
      stop / secondsInAnHour, 
      rr[0], 
      rr[1], 
      rr[1] - rr[0] 
    ),
    
    if stop > start: 
      print "pos"
      
      r = np.linspace( start, stop, ( stop - start ) / rscale )
      x.extend( ( 24 * r / secondsInAnHour ).tolist() )
      
    else: 
      print ""
      
      r = np.linspace( start, 24.0, ( 24.0 - start ) / rscale )
      #x.extend( ( 24 * r / secondsInAnHour ).tolist() )
      
      r = np.linspace( 0, stop, ( stop - 0 ) / rscale )
      #x.extend( ( 24 * r / secondsInAnHour ).tolist() )
    
  print "", len( x ), "seconds"
  
  perm = np.random.permutation( len( x ) )
  x = np.asarray( x ) / 24
  
  json.dump( x[perm].tolist(), open( "%s_td.json" % tn, "w" ) )
  
  print 
  
  if len( x ) < 10:
    continue
  
  print "plotting"
  pl.figure()
  pl.hist( x, bins = np.linspace( 0, 24, 48 ), normed = True )
  pl.xlabel( "Hour of day" )
  pl.ylabel( "Density" )
  pl.title( "%s (%d events)" % ( tn, len( x ) ) )
  pl.xlim( ( 0, 24 ) )
  pl.savefig( "%s_td.png" % tn )
  
  print
  
