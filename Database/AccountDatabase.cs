﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class AccountDatabase
{
    public const string accountDataFile = "AccountData.data";
    public const string heroNameDatafile = "HeroNameData.data";

    private static AccountDatabase instance;

    public static AccountDatabase Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new AccountDatabase();
                return instance;
            }
            else
            {
                return instance;
            }
        }
    }

    Hashtable accountData;
    Hashtable userData;
    List<string> heroNameData;
    BinaryFormatter bin;

    public Hashtable AccountData
    {
        get
        {
            if (accountData == null)
            {
                FileStream fs = new FileStream(accountDataFile, FileMode.OpenOrCreate);

                try
                {
                    if (fs.Length > 0)
                    {
                        accountData = (Hashtable)bin.Deserialize(fs);
                        fs.Close();
                        return accountData;
                    }
                    else
                    {
                        fs.Close();
                        return new Hashtable();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Database::AccountData.GetData 에러 " + e.Message);
                    fs.Close();
                    return new Hashtable();
                }
            }
            else
            {
                return accountData;
            }
        }
    }
    public Hashtable UserData { get { return userData; } }
    public List<string> HeroNameData
    {
        get
        {
            if (heroNameData == null)
            {
                FileStream fs = new FileStream(heroNameDatafile, FileMode.OpenOrCreate);

                try
                {
                    if (fs.Length > 0)
                    {
                        heroNameData = (List<string>)bin.Deserialize(fs);
                        fs.Close();
                        return heroNameData;
                    }
                    else
                    {
                        fs.Close();
                        return new List<string>();
                    }
                }
                catch
                {
                    Console.WriteLine("Database::HeroNameData.GetData 에러");

                    fs.Close();
                    return null;
                }
            }
            else
            {
                return heroNameData;
            }
        }
    }

    //초기화
    public void InitailizeDatabase()
    {
        bin = new BinaryFormatter();

        accountData = AccountData;
        heroNameData = HeroNameData;
        userData = new Hashtable();
    }

    //가입시 아이디 추가
    public bool AddAccountData(AccountData newAccountData)
    {
        try
        {
            if (!accountData.Contains(newAccountData.Id))
            {
                accountData.Add(newAccountData.Id, newAccountData);
                FileSave(accountDataFile, accountData);
                FileSave(newAccountData.Id + ".data", new UserData(newAccountData.Id));

                return true;
            }
            else
            {
                Console.WriteLine("이미 존재하는 아이디");
                return false;
            }
        }
        catch
        {
            Console.WriteLine("Database::AddUserData.Add 에러");
            return false;
        }
    }

    //탈퇴시 아이디 삭제
    public Result DeleteAccountData(string id, string pw)
    {
        try
        {
            if (accountData.Contains(id))
            {
                if (((AccountData)accountData[id]).Password == pw)
                {
                    accountData.Remove(id);
                    FileSave(accountDataFile, accountData);

                    FileInfo file = new FileInfo(id + ".data");
                    file.Delete();

                    return Result.Success;
                }
                else
                {
                    Console.WriteLine("잘못된 비밀번호");
                    return Result.Fail;
                }
            }
            else
            {
                Console.WriteLine("존재하지 않는 아이디");
                return Result.Fail;
            }
        }
        catch
        {
            Console.WriteLine("Database::DeleteAccountData.Contains 에러");
            return Result.Fail;
        }
    }

    public UserData GetUserData(string id)
    {
        if (userData.Contains(id))
        {
            return (UserData)userData[id];
        }

        FileStream fs = new FileStream(id + ".data", FileMode.Open);
        UserData newUserData = null;

        if (fs.Length > 0)
        {
            newUserData = (UserData)bin.Deserialize(fs);
        }
        else
        {
            Console.WriteLine("Database::GetUserData.Deserialize 에러");
        }

        fs.Close();
        return newUserData;
    }

    //로그인 시 유저 데이터 추가
    public bool AddUserData(string id)
    {
        try
        {
            UserData newUserData = GetUserData(id);
            userData.Add(id, newUserData);

            return true;
        }
        catch
        {
            Console.WriteLine("Database::AddUserData.Deserialize 에러");
            return false;
        }
    }

    //로그아웃 시 유저 데이터 제거
    public bool DeleteUserData(string id)
    {
        try
        {
            userData.Remove(id);
            return true;
        }
        catch
        {
            Console.WriteLine("Database::DeleteUserData.Remove 에러");
            return false;
        }        
    }
    
    //영웅 데이터 반환
    public HeroData GetHeroData(string id, int characterId)
    {
        UserData userData = GetUserData(id);
        return userData.HeroData[characterId];
    }

    //파일 저장
    public bool FileSave(string path, object data)
    {
        FileStream fs = new FileStream(path, FileMode.Create);

        try
        {
            bin.Serialize(fs, data);
        }
        catch (Exception e)
        {
            Console.WriteLine("Database::FileSaveSerialize 에러" + e.Message);
            fs.Close();
            return false;
        }

        fs.Close();
        return true;
    }
}