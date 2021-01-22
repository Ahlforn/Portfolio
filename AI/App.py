import Exercise1.MotionDetection as md
import Exercise1.PedestrianDetection as pd
import Exercise1.FaceDetection as fd

program_state = 0
program_states = ['Motion Detection', 'Pedestrian Detection', 'Face Detection']
url = 'https://youtu.be/aUdKzb4LGJI'

if __name__ == '__main__':
    while True:
        key_pressed = None

        if program_state == 0:
            motion_detection = md.MotionDetection()
            key_pressed = motion_detection.start()

        if program_state == 1:
            pedestrian = pd.PedestrianDetection(url)
            key_pressed = pedestrian.start()

        if program_state == 2:
            face_detection = fd.FaceDetection()
            key_pressed = face_detection.start()

        if key_pressed == 27:
            break

        if key_pressed == 123:
            if program_state == 0:
                program_state = len(program_states) - 1
            else:
                program_state = program_state - 1

        if key_pressed == 124:
            if program_state == len(program_states) - 1:
                program_state = 0
            else:
                program_state = program_state + 1
