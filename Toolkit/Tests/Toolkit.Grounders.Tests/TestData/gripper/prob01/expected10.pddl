(:action pick
	:parameters (ball2 rooma right)
	:precondition  
		(and  
			(ball ball2) 
			(room rooma) 
			(gripper right)
			(at ball2 rooma) 
			(at-robby rooma) 
			(free right)
		)
	:effect 
		(and 
			(carry ball2 right)
		    (not (at ball2 rooma)) 
		    (not (free right))
		)
	)