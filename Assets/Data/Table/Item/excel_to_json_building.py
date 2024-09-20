import pandas as pd
import sys
import os
import json
import math

def excel_to_custom_json(input_file, output_file):
    try:
        # 첫 번째 줄을 건너뛰고 엑셀 파일을 읽기
        df = pd.read_excel(input_file, skiprows=[1])

        # 결과를 저장할 dictionary 생성
        result = {"dic": {}}

        # 각 행을 순회하며 JSON 구조에 맞게 변환
        for _, row in df.iterrows():
            id = row["Id"]

            # NaN 값 처리를 위해 math.isnan() 또는 pd.isna()로 처리
            result["dic"][id] = {
                "Id": id,
                "BuildingType": row["BuildingType"],
                "RecipyGroup": row["RecipyGroup"] if pd.notna(row["RecipyGroup"]) else None,  # NaN 처리
                "DeploySizeX": row["DeploySizeX"] if pd.notna(row["DeploySizeX"]) else 0,  # NaN 처리
                "DeploySizeY": row["DeploySizeY"] if pd.notna(row["DeploySizeY"]) else 0,  # NaN 처리
                "DeploySizeZ": row["DeploySizeZ"] if pd.notna(row["DeploySizeZ"]) else 0,  # NaN 처리
                "UseEnergy": True if row["UseEnergy"] == 1.0 else False,                     # 부울값 처리
                "EnergyEfficiency": float(f"{row['EnergyEfficiency']:.1f}") if pd.notna(row["EnergyEfficiency"]) else 1.0,  # 소수점 1자리까지 강제 유지
                "SpeedEfficiency": float(f"{row['SpeedEfficiency']:.1f}") if pd.notna(row["SpeedEfficiency"]) else 1.0,  # 소수점 1자리까지 강제 유지
            }

        # 결과를 JSON 파일로 저장
        with open(output_file, "w", encoding="utf-8") as json_file:
            json.dump(result, json_file, ensure_ascii=False, indent=2)

        print(f"File converted successfully: {output_file}")

    except Exception as e:
        print(f"Error occurred: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python excel_to_custom_json.py <input_excel_file> <output_json_file>")
    else:
        input_file = sys.argv[1]
        output_file = sys.argv[2]
        excel_to_custom_json(input_file, output_file)