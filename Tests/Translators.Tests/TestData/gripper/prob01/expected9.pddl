(:action pick
	:parameters (ball2 rooma left)
	:precondition  
		(and  
			(ball ball2) 
			(room rooma) 
			(gripper left)
			(at ball2 rooma) 
			(at-robby rooma) 
			(free left)
		)
	:effect 
		(and 
			(carry ball2 left)
		    (not (at ball2 rooma)) 
		    (not (free left))
		)
	)