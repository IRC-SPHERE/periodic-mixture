import numpy as np
import json
import time 
import matplotlib.pyplot as pl 

import sys

x = json.load( open( sys.argv[1] ) )
  
pl.figure()
pl.hist( x, bins = np.linspace( 0, 24, 48 ), normed = True )
pl.xlabel( "Hour of day" )
pl.ylabel( "Density" )
pl.title( sys.argv[1] )
pl.xlim( ( 0, 24 ) )
  
pl.show()
