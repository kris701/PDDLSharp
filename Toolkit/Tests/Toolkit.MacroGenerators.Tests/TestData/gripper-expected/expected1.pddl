(:action move-drop
    :parameters (?ball1 ?rooma ?roomb ?left)
    :precondition  (and 
			(carry ?ball1 ?left)
			(at-robby ?rooma) 
			(ball ?ball1)
			(room ?rooma)
			(room ?roomb)
			(gripper ?left)
			)
    :effect (and 
			(at ?ball1 ?roomb)
			(free ?left)
			(not (carry ?ball1 ?left))
			(not (at-robby ?rooma))
			(at-robby ?roomb) 
		)
	)