import concurrent.futures
import datetime
import random
import subprocess
import sys


def generate_route_arguments(vehicle_id: int) -> str:
    start_points = ['48.178851,16.378870', '48.190457,15.642981', '46.049649,14.519736', '44.856964,13.864281',
                    '41.287753,19.824857', '39.620098,22.413780', '37.961374,23.752639', '42.690997,23.370342',
                    '44.404488,26.150189', '47.000184,28.864902', '47.959345,21.711443', '47.483509,19.093085',
                    '48.703587,21.255383', '50.042517,19.952157', '50.800820,19.123734', '51.751902,19.463886',
                    '53.106708,18.039726', '54.332546,18.648856', '53.414103,14.539195', '53.581652,10.093996',
                    '53.087065,8.856769', '53.208394,6.502340', '52.352530,4.619789', '52.045572,4.215902',
                    '51.038200,3.709885', '50.817473,4.331092', '49.419245,11.086757', '48.081446,11.649786',
                    '47.237185,9.588635', '45.399371,11.897633']
    end_points = ['51.423358,6.740340', '51.217770,6.779526', '50.918915,6.954502', '50.687730,7.141306',
                  '50.091873,8.676828', '44.501565,11.291342', '41.865551,12.492700', '40.846106,14.247557',
                  '40.001017,15.369910', '39.556347,16.120833', '38.672738,16.099535', '41.963672,2.828238',
                  '41.353901,2.078050', '39.450877,-0.377842', '37.967260,-1.114924', '36.713687,-4.444215',
                  '38.872464,-6.967657', '38.743545,-9.336464', '41.110540,-8.592430', '43.245027,-2.899443',
                  '47.189086,-1.561229', '47.074345,2.419941', '48.902831,2.503302', '48.683285,6.185777',
                  '52.264308,8.059897', '52.356261,9.748238', '52.504423,13.402352', '43.832968,18.347183',
                  '45.803679,15.996344', '44.796398,20.479196']
    now = datetime.datetime.now()

    random_start_point = start_points[random.randint(0, len(start_points) - 1)]
    random_end_point = end_points[random.randint(0, len(start_points) - 1)]
    time_delta = datetime.timedelta(days=random.randint(0, 730), hours=random.randint(0, 12),
                                    minutes=random.randint(0, 60))
    random_start_date = (now - time_delta).strftime("%Y-%m-%dT%H:%M:%S")
    return f'-s {random_start_point} -e {random_end_point} -v {vehicle_id} -t {random_start_date}'


def generate_track_utility_commands(track_generator_executable):
    first_vehicle_id = 21
    last_vehicle_id = 15020
    vehicles_sample_size = 200
    routes_per_vehicle = 20

    random_vehicle_ids = [random.randint(first_vehicle_id, last_vehicle_id) for _ in range(vehicles_sample_size)]
    return [f'{track_generator_executable} {generate_route_arguments(vehicle_id)}'
            for vehicle_id in random_vehicle_ids for _ in range(routes_per_vehicle)]


def execute_command(command: str, command_idx: int):
    try:
        result = subprocess.run(command, shell=True, check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        print(f'{command_idx + 1}) {result.stdout.decode()}')
    except subprocess.CalledProcessError as e:
        print(e.stderr.decode())


def main(track_generator_executable: str):
    utility_commands = generate_track_utility_commands(track_generator_executable)
    with concurrent.futures.ThreadPoolExecutor(max_workers=10) as executor:
        command_futures = [executor.submit(execute_command, command, command_idx) for command_idx, command in
                           enumerate(utility_commands)]
        concurrent.futures.wait(command_futures, return_when=concurrent.futures.ALL_COMPLETED)


if __name__ == '__main__':
    main(sys.argv[1])
