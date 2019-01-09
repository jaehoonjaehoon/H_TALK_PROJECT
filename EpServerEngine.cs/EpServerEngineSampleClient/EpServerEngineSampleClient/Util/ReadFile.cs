using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace Util
{
    class ReadFile
    {
        // Text File을 읽으면서 2개의 변수에 대해 Set을 할 때 사용 하는 함수.
        // val1이 첫번째로 set, val2가 두번째로 set 된다.
        public static bool ReadFileAndSetValue( string path, string fileName, ref string val1, ref string val2 )
        {

            DirectoryInfo di = new DirectoryInfo(path);

            if( true == di.Exists)
            {
                string filePath = path + fileName;
                FileInfo fi = new FileInfo(filePath);
                if( true == fi.Exists )
                {
                    StreamReader readFile = new StreamReader(filePath);
                   
                    val1 = readFile.ReadLine();
                    val2 = readFile.ReadLine();

                }
                else if( false == fi.Exists)
                {
                    MessageBox.Show(String.Format("{0}의 파일이 존재하지 않습니다.", fileName), "경고", MessageBoxButtons.OK);

                }
            }
            else if( false == di.Exists )
            {
                MessageBox.Show(String.Format("{0}의 경로가 존재하지 않습니다.",path), "경고", MessageBoxButtons.OK);
            }
            return true;
        }
    }
}
