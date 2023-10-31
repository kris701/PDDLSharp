(:action pick
	:parameters (ball1 rooma right)
	:precondition  
		(and  
			(ball ball1) 
			(room rooma) 
			(gripper right)
			(at ball1 rooma) 
			(at-robby rooma) 
			(free right)
		)
	:effect 
		(and 
			(carry ball1 right)
		    (not (at ball1 rooma)) 
		    (not (free right))
		)
	)