# excel_to_json.py
import pandas as pd
import sys
import os

def excel_to_json(input_file, output_file):
    try:
        # 엑셀 파일을 pandas 데이터프레임으로 읽기
        df = pd.read_excel(input_file)

        # 데이터프레임을 JSON 파일로 변환하여 저장
        df.to_json(output_file, orient='records', force_ascii=False, indent=4)
        print(f"File converted successfully: {output_file}")
    except Exception as e:
        print(f"Error occurred: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python excel_to_json.py <input_excel_file> <output_json_file>")
    else:
        input_file = sys.argv[1]
        output_file = sys.argv[2]
        excel_to_json(input_file, output_file)